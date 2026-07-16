import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import * as matchApi from "../matches/matchApi";
import * as matchPredictionApi from "../matchPredictions/matchPredictionApi";
import {
  MATCH_STATUS,
  type CreateMatchPredictionRequest,
  type MatchPredictionPreviewDto,
  type MatchPreviewDto,
  type UpdateMatchPredictionRequest,
} from "../matches/matchTypes";

type FixturesStatus = "idle" | "loading" | "ready" | "error";

type FixturesState = {
  status: FixturesStatus;
  seasonId: string | null;
  matches: MatchPreviewDto[];
  predictionsByMatchId: Record<string, MatchPredictionPreviewDto>;
  savingMatchIds: string[];
};

const initialState: FixturesState = {
  status: "idle",
  seasonId: null,
  matches: [],
  predictionsByMatchId: {},
  savingMatchIds: [],
};

function toPredictionMap(
  predictions: MatchPredictionPreviewDto[],
): Record<string, MatchPredictionPreviewDto> {
  return Object.fromEntries(predictions.map((prediction) => [prediction.matchId, prediction]));
}

function selectFixturesForDisplay(matches: MatchPreviewDto[]): MatchPreviewDto[] {
  const now = Date.now();

  return matches
    .filter((match) => {
      if (match.status === MATCH_STATUS.Postponed) {
        return true;
      }

      if (match.status === MATCH_STATUS.Scheduled) {
        return new Date(match.kickoffTime).getTime() > now;
      }

      return false;
    })
    .sort(
      (left, right) =>
        new Date(left.kickoffTime).getTime() - new Date(right.kickoffTime).getTime(),
    );
}

export const loadFixturesForSeason = createAsyncThunk(
  "fixtures/loadFixturesForSeason",
  async (competitionSeasonId: string, { rejectWithValue }) => {
    const [matchesResult, predictionsResult] = await Promise.all([
      matchApi.getMatchesBySeason(competitionSeasonId),
      matchPredictionApi.getMyPredictionsBySeason(competitionSeasonId),
    ]);

    if (!matchesResult.success || !matchesResult.data) {
      return rejectWithValue(matchesResult.message ?? "Maçlar yüklenemedi.");
    }

    return {
      seasonId: competitionSeasonId,
      matches: selectFixturesForDisplay(matchesResult.data),
      predictions: predictionsResult.success && predictionsResult.data ? predictionsResult.data : [],
    };
  },
);

export const saveMatchPrediction = createAsyncThunk(
  "fixtures/saveMatchPrediction",
  async (
    input: {
      matchId: string;
      predictionId: string | null;
      predictedHomeScore: number;
      predictedAwayScore: number;
      predictedAdvancingTeamId: string | null;
    },
    { rejectWithValue },
  ) => {
    if (input.predictionId) {
      const request: UpdateMatchPredictionRequest = {
        id: input.predictionId,
        predictedHomeScore: input.predictedHomeScore,
        predictedAwayScore: input.predictedAwayScore,
        predictedAdvancingTeamId: input.predictedAdvancingTeamId,
      };

      const result = await matchPredictionApi.updatePrediction(request);

      if (!result.success) {
        return rejectWithValue(result.message ?? "Tahmin güncellenemedi.");
      }

      return {
        matchId: input.matchId,
        prediction: {
          id: input.predictionId,
          matchId: input.matchId,
          predictedHomeScore: input.predictedHomeScore,
          predictedAwayScore: input.predictedAwayScore,
          predictedAdvancingTeamId: input.predictedAdvancingTeamId,
          pointsEarned: null,
        } satisfies Omit<MatchPredictionPreviewDto, "match">,
      };
    }

    const request: CreateMatchPredictionRequest = {
      matchId: input.matchId,
      predictedHomeScore: input.predictedHomeScore,
      predictedAwayScore: input.predictedAwayScore,
      predictedAdvancingTeamId: input.predictedAdvancingTeamId,
    };

    const result = await matchPredictionApi.createPrediction(request);

    if (!result.success || !result.data) {
      return rejectWithValue(result.message ?? "Tahmin kaydedilemedi.");
    }

    return {
      matchId: input.matchId,
      prediction: {
        id: result.data.id,
        matchId: input.matchId,
        predictedHomeScore: input.predictedHomeScore,
        predictedAwayScore: input.predictedAwayScore,
        predictedAdvancingTeamId: input.predictedAdvancingTeamId,
        pointsEarned: null,
      } satisfies Omit<MatchPredictionPreviewDto, "match">,
    };
  },
);

const fixturesSlice = createSlice({
  name: "fixtures",
  initialState,
  reducers: {
    clearFixtures(state) {
      state.status = "idle";
      state.seasonId = null;
      state.matches = [];
      state.predictionsByMatchId = {};
      state.savingMatchIds = [];
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadFixturesForSeason.pending, (state) => {
        state.status = "loading";
      })
      .addCase(loadFixturesForSeason.fulfilled, (state, action) => {
        state.status = "ready";
        state.seasonId = action.payload.seasonId;
        state.matches = action.payload.matches;
        state.predictionsByMatchId = toPredictionMap(action.payload.predictions);
      })
      .addCase(loadFixturesForSeason.rejected, (state) => {
        state.status = "error";
        state.matches = [];
        state.predictionsByMatchId = {};
      })
      .addCase(saveMatchPrediction.pending, (state, action) => {
        state.savingMatchIds.push(action.meta.arg.matchId);
      })
      .addCase(saveMatchPrediction.fulfilled, (state, action) => {
        state.savingMatchIds = state.savingMatchIds.filter(
          (matchId) => matchId !== action.payload.matchId,
        );

        const match = state.matches.find((item) => item.id === action.payload.matchId);
        if (!match) {
          return;
        }

        state.predictionsByMatchId[action.payload.matchId] = {
          ...action.payload.prediction,
          match,
        };
      })
      .addCase(saveMatchPrediction.rejected, (state, action) => {
        state.savingMatchIds = state.savingMatchIds.filter(
          (matchId) => matchId !== action.meta.arg.matchId,
        );
      });
  },
});

export const { clearFixtures } = fixturesSlice.actions;
export default fixturesSlice.reducer;
